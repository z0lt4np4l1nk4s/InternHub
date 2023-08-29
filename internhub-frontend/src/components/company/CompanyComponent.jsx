import React from "react";
import { useNavigate } from "react-router-dom";
import { CompanyService } from "../../services";
import Button from "../Button";

export default function CompanyComponent({
  company,
  onRemove,
  isActive = true,
}) {
  const navigate = useNavigate();
  const companyService = new CompanyService();

  return (
    <tr>
      <td>{company.getFullName()}</td>
      <td>{company.name}</td>
      <td>{company.address}</td>
      <td>
        <a href={company.website} target="_blank" rel="noreferrer">
          {company.website}
        </a>
      </td>
      <td>
        {isActive && (
          <>
            <Button
              buttonColor="primary"
              onClick={() => navigate(`/company/details/${company.id}`)}
            >
              Details
            </Button>
            <Button
              buttonColor="secondary"
              onClick={() => navigate(`/company/edit/${company.id}`)}
            >
              Edit
            </Button>
            <Button
              buttonColor="danger"
              onClick={async () => {
                const result = window.confirm(
                  "Are you sure that you want to delete this company?"
                );
                if (result) {
                  const removeResult = await companyService.removeAsync(
                    company.id
                  );
                  if (removeResult) onRemove();
                  else
                    alert(
                      "An error occured while removing the company... Please try again later!"
                    );
                }
              }}
            >
              Delete
            </Button>
          </>
        )}
      </td>
    </tr>
  );
}
