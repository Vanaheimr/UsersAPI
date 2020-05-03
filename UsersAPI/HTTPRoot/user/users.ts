///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />
///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/defaults/search.ts" />

function StartUserSearch(NoURIupdate?: boolean) {

    StartSearch2<IUserProfile>("/users",
                               "user",
                               "users",
                               (user, userDiv) => {

                                   CreateDiv(userDiv, "id",    user["@id"]);
                                   CreateDiv(userDiv, "name",  user.name);
                                   CreateDiv(userDiv,  null,   CreateI18NDiv(user.description, "description"));
                                   CreateDiv(userDiv, "email", "<a href=\"" + user.email + "\">" + user.email + "</a>");

                                   //let propertiesDiv = searchResultDiv.appendChild(document.createElement('div'));
                                   //propertiesDiv.className = "properties";

                                   //AddProperty(propertiesDiv, "model",
                                   //            "Model", User.hardware != null
                                   //                         ? User.hardware.model
                                   //                         : "unknown");

                                   //AddProperty(propertiesDiv, "adminStatus",
                                   //            "(Admin-)Status", firstValue(User.adminStatus) + " / " + firstValue(User.status));

                                   //if (User.dailySelfTests != null || User.monthlySelfTests != null)
                                   //{

                                   //    AddProperty(propertiesDiv, "selfTest",
                                   //                "Last self test", firstKey(User.monthlySelfTests) > firstKey(User.dailySelfTests)
                                   //                                      ? "MONTHLY " + firstKey(User.monthlySelfTests)
                                   //                                      : "DAILY "   + firstKey(User.dailySelfTests));

                                   //}

                               },
                               null,
                               false,
                               null,
                               NoURIupdate);

}
