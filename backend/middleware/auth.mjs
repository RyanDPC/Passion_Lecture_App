import jwt from 'jsonwebtoken';
import dotenv from 'dotenv';
dotenv.config();
const auth = (req, res, next) => {
  //vérifie que l'utilisateur possède un token stocké dans les cookies
  const token = req.cookies.token;
  if (!token) {
    return res
      .status(400)
      .json({ message: "Vous n'avez pas fourni de jeton d'authentification" });
  }
  jwt.verify(token, process.env.privateKey, (error, decodedToken) => {
    if (error) {
      const message = `L'utilisateur n'est pas autorisé à accéder à cette ressource.`;
      return res.status(401).json({ message, data: error });
    }
    //vérifie si il y a un userId dans la requete et qu'elle ne
    //correspond pas a celle du token cela signifie que l'utilisateur
    //essaie d'accéder à une ressource qui ne lui appartient pas.
    const id = decodedToken.userId;
    if (req.body.id && req.body.id !== id) {
      const message = `L'identifiant de l'utilisateur est invalide`;
      return res.status(401).json({ message });
    } else {
      req.user = decodedToken;
      next();
    }
  });
};
const validateToken = (req, res) => {
  try {
    const token = req.cookies.token;
    if (!token) {
      return res.status(204).json({ message: 'Pas de token' });
    }
    jwt.verify(token, process.env.privateKey, (error, decodedToken) => {
      if (error) {
        return res.status(204).json({ message: 'Token invalide' });
      }
      return res.status(200).json({ message: 'Token valide' });
    });
  } catch (err) {
    console.error(err);
  }
};
export { auth, validateToken };
