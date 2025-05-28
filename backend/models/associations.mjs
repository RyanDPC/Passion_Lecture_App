const initAssociations = async (
  User,
  Editor,
  Comment,
  Category,
  Book,
  Author,
  Tag
) => {
  User.hasMany(Comment, {
    foreignKey: "user_fk",
  });
  Comment.belongsTo(User, {
    foreignKey: "user_fk",
  });
  Book.hasMany(Comment, {
    foreignKey: "book_fk",
  });
  Comment.belongsTo(Book, {
    foreignKey: "book_fk",
  });
  Book.belongsTo(Category, {
    foreignKey: "category_fk",
  });
  Category.hasMany(Book, {
    foreignKey: "category_fk",
  });
  Book.belongsTo(Editor, {
    foreignKey: "editor_fk",
  });
  Editor.hasMany(Book, {
    foreignKey: "editor_fk",
  });
  Author.hasMany(Book, {
    foreignKey: "author_fk",
  });
  Book.belongsTo(Author, {
    foreignKey: "author_fk",
  });
  Book.belongsTo(User, {
    foreignKey: "user_fk",
  });
  User.hasMany(Book, {
    foreignKey: "user_fk",
  });
  User.hasOne(Author, { foreignKey: "user_fk" });
  Author.belongsTo(User, { foreignKey: "user_fk" });

  Author.hasMany(Book, { foreignKey: "author_fk" });
  Book.belongsTo(Author, { foreignKey: "author_fk" });
Book.belongsToMany(Tag, {
  through: 't_book_tag',
  foreignKey: 'book_id',
  otherKey: 'tag_id',
});
Tag.belongsToMany(Book, {
  through: 't_book_tag',
  foreignKey: 'tag_id',
  otherKey: 'book_id',
});
};
export { initAssociations };
