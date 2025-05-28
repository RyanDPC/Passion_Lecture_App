const AuthorModel = (sequelize, DataTypes) => {
  return sequelize.define("t_author", {
    id: {
      type: DataTypes.INTEGER,
      primaryKey: true,
      autoIncrement: true,
    },
    firstname: {
      type: DataTypes.STRING,
      allowNull: false,
      validate: {
        notNull: {
          msg: "Le prénom de l'écrivain est une propriété obligatoire",
        },
      },
    },
    lastname: {
      type: DataTypes.STRING,
      allowNull: false,
      validate: {
        notNull: {
          msg: "Le nom de famille de l'écrivain est une propriété obligatoire",
        },
      },
    },
    user_fk: {
      type: DataTypes.INTEGER,
      allowNull: true,
      references: {
        model: "t_user",
        key: "id",
      },
    },
  });
};

export { AuthorModel };
