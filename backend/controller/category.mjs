import { Category, Book, Comment, Author } from "../db/sequelize.mjs";
import { ValidationError } from "sequelize";
// Ajouter une catégorie
export function Create(req, res) {
  const { name } = req.body;
  Category.create({
    name,
  })
    .then((category) => {
      res.status(201).json(category);
    })
    .catch((e) => {
      //si c'est une erreur de validation renvoie le messgae personnalisé
      if (e instanceof ValidationError) {
        return res.status(400).json({ message: e.message });
      }
    });
}
export function Delete(req, res) {
  Category.findByPk(req.params.id).then((deletedcategory) => {
    if (!deletedcategory) {
      const message =
        "Le produit demandé n'existe pas. Merci de réessayer avec un autre identifiant.";
      return res.status(404).json({ message });
    }
    return Category.destroy({
      where: { id: deletedcategory.id },
    }).then((_) => {
      const message = `La categorie ${deletedcategory.name} a bien été supprimé !`;
      res.status(201).json({ message, deletedcategory });
    });
  });
}

// Récupérer toutes les catégories avec le nombre de livres
export function All(req, res) {
  Category.findAll({
    attributes: ["id", "name"],
    include: [
      {
        model: Book,
        attributes: [], // On ne récupère pas de détails sur le livre
      },
    ],
  })
    .then((categories) => {
      res.status(200).json({ categories });
    })
    .catch((error) => {
      console.error(error);
      res.status(500).json({
        message: "Erreur lors de la récupération des catégories",
        error,
      });
    });
}

// Trouver un livre par sa catégorie
export function FindByCategory(req, res) {
  const { id } = req.params;
  Category.findByPk(id, {
    include: [{
      model: Book,
      attributes: ['id','name','coverImage'],
      include: [
        {
          model: Comment
        },
        {
          model: Author,
          attributes: ['id', 'firstname', 'lastname']
        }
      ]
    }]
  })
    .then((category) => {
      if (!category) {
        return res.status(204).json({
          message: "La catégorie n'existe pas",
        });
      }

      // Calculer la moyenne des notes pour chaque livre
      const booksWithAvg = category.t_books.map(book => {
        const notes = book.t_comments.map(comment => comment.note);
        const avg = notes.length > 0 ? notes.reduce((acc, val) => acc + val, 0) / notes.length : 1;
        return {
          ...book.toJSON(),
          avg
        };
      });

      res.status(200).json({
        ...category.toJSON(),
        t_books: booksWithAvg
      });
    })
    .catch((error) => {
      return res.status(500).json({
        message: "Erreur lors de la recherche de la catégorie",
        error,
      });
    });
}
