import React from "react";
import { CompanyService } from "../../services";
import Button from "../Button";
import { useNavigate } from "react-router-dom";

export default function CompanyAdminComponent({ company, onChange }) {
  const navigate = useNavigate();
  const companyService = new CompanyService();

  async function onClick(isAccepted) {
    const result = await companyService.approveAsync(company.id, isAccepted);
    if (result) {
      onChange();
    }
  }
  return (
    <tr>
      <td>{company.getFullName()}</td>
      <td>{company.name}</td>
      <td>{company.address}</td>
      <td>
        <a href={company.website} target="_blank">
          {company.website}
        </a>
      </td>
      <td>
        <Button
          buttonColor="primary"
          onClick={async () => navigate("/company/details/" + company.id)}
        >
          Details
        </Button>
        <Button buttonColor="success" onClick={async () => await onClick(true)}>
          Accept
        </Button>
        <Button buttonColor="danger" onClick={async () => await onClick(false)}>
          Decline
        </Button>
      </td>
    </tr>
  );
}
