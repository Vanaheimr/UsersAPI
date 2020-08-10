///<reference path="../../../../UsersAPI/UsersAPI/HTTPRoot/libs/date.format.ts" />

function StartOrganizationSearch(NoURIupdate?: boolean) {

    StartSearch<IOrganization>("/organizations",
                               "organization",
                               "organizations",
                               (organization, organizationDiv) => {
                                    CreateDiv(organizationDiv, "id", organization["@id"]);
                                    organizationDiv.appendChild(CreateI18NDiv(organization.name,         "name"));
                                    organizationDiv.appendChild(CreateI18NDiv(organization.description,  "description"));
                                },
                                null,
                                organization => "",
                                null);

}
