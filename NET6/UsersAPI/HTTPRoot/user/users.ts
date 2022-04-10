/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
/////<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/defaults/search.ts" />

function StartUserSearch() {

    StartSearch<IUserProfile>("/users",
                              "user",
                              "users",
                              "users",
                              (user, userDiv) => {
                                  CreateDiv(userDiv, "id",    user["@id"]);
                                  CreateDiv(userDiv, "name",  user.name);
                                  CreateDiv(userDiv,  null,   CreateI18NDiv(user.description, "description"));
                                  CreateDiv(userDiv, "email", "<a href=\"" + user.email + "\">" + user.email + "</a>");
                              },
                              null,
                              user => "",
                              null);

}
