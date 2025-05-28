const BookModel = (sequelize, DataTypes) => {
  return sequelize.define(
    "t_book",
    {
      id: {
        type: DataTypes.INTEGER,
        primaryKey: true,
        autoIncrement: true,
      },
      name: {
        type: DataTypes.STRING,
        allowNull: false,
        unique: {
          msg: "Ce nom est déjà pris.",
        },
        validate: {
          is: {
            args: /^[\p{L}\p{P}\s\d]*$/u,
            msg: "Seules les lettres, les accents, les espaces, les virgules et les points sont autorisés",
          },
          notEmpty: {
            msg: "Le nom ne peut pas être vide.",
          },
          notNull: {
            msg: "Le nom est une propriété obligatoire",
          },
        },
      },
      passage: {
        type: DataTypes.STRING(250),
        validate: {
          is: {
            args: /^[\p{L}\p{P}\s\d]*$/u,
            msg: "Seules les lettres, les accents, les espaces, les virgules et les points sont autorisés",
          },
          len: {
            args: [0, 250],
            msg: "Le passage ne peut pas dépasser 250 caractères",
          },
        },
      },
      summary: {
        type: DataTypes.STRING(2000),
        allowNull: false,
        validate: {
          len: {
            args: [1, 2000],
            msg: "Le résumé doit faire entre 1 et 2000 caractères",
          },
        },
      },
      editionYear: {
        type: DataTypes.INTEGER,
        allowNull: false,
        validate: {
          isInt: {
            args: true,
            msg: "L'année doit être un entier valide.",
          },
          max: {
            args: new Date().getFullYear(),
            msg: "L'année ne peut pas être dans le futur.",
          },
          notNull: {
            msg: "L'année d'edition est une propriété obligatoire",
          },
        },
      },
      coverImage: {
        type: DataTypes.BLOB("long"),
      },
      pages: {
        type: DataTypes.INTEGER,
        allowNull: true,
        },
        content: {
        type: DataTypes.BLOB("long"),
        allowNull: true,
      },
      isRead: {
        type: DataTypes.BOOLEAN,
        allowNull: false,
        defaultValue: false,
      },
    },
    {
      timestamps: true,
      createdAt: "created",
      updatedAt: false,
    }
  );
};
export { BookModel };
