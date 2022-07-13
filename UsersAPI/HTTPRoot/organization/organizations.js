///<reference path="../libs/date.format.ts" />
function StartOrganizationSearch() {
    StartSearch("/organizations", "organization", "organizations", "organizations", (organization, organizationDiv) => {
        CreateDiv(organizationDiv, "id", organization["@id"]);
        organizationDiv.appendChild(CreateI18NDiv(organization.name, "name"));
        organizationDiv.appendChild(CreateI18NDiv(organization.description, "description"));
    }, null, organization => "", null);
}
//# sourceMappingURL=organizations.js.map