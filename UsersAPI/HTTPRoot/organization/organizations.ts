///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartOrganizationSearch(NoURIupdate?: boolean) {

    StartSearch2<IOrganization>("/organizations",
                                "organization",
                                "organizations",
                                (organization, organizationDiv) => {

                                    // show id
                                    CreateDiv(organizationDiv, "id", organization["@id"]);

                                    // show name
                                    organizationDiv.appendChild(CreateI18NDiv(organization.name,         "name"));

                                    // show description
                                    organizationDiv.appendChild(CreateI18NDiv(organization.description,  "description"));

                                    //let descriptionOrIdDiv  = searchResultDiv.appendChild(document.createElement('div'));
                                    //descriptionOrIdDiv.className  = "I18N descriptionOrId";
                                    //descriptionOrIdDiv.innerHTML  = organization.description != null && firstValue(organization.description) != ""
                                    //                                    ? ShowI18N(organization.description)
                                    //                                    : "<p>" + organization["@id"] + "</p>";



                                    //let propertiesDiv = searchResultDiv.appendChild(document.createElement('div'));
                                    //propertiesDiv.className = "properties";

                                    //AddProperty(propertiesDiv, "model",
                                    //            "Model", organization.hardware != null
                                    //                         ? organization.hardware.model
                                    //                         : "unknown");

                                    //AddProperty(propertiesDiv, "adminStatus",
                                    //            "(Admin-)Status", firstValue(organization.adminStatus) + " / " + firstValue(organization.status));

                                    //if (organization.dailySelfTests != null || organization.monthlySelfTests != null)
                                    //{

                                    //    AddProperty(propertiesDiv, "selfTest",
                                    //                "Last self test", firstKey(organization.monthlySelfTests) > firstKey(organization.dailySelfTests)
                                    //                                      ? "MONTHLY " + firstKey(organization.monthlySelfTests)
                                    //                                      : "DAILY "   + firstKey(organization.dailySelfTests));

                                    //}

                                },
                                null,
                                false,
                                null,
                                NoURIupdate);

}
