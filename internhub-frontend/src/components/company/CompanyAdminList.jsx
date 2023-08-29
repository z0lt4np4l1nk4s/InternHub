import React from "react";
import Table from "../Table";
import CompanyAdminComponent from "./CompanyAdminComponent";

export default function CompanyAdminList({ companies, onChange }) {
  return (
    <Table>
      <thead>
        <tr>
          <td>Full Name</td>
          <td>Company Name</td>
          <td>Address</td>
          <td>Website</td>
          <td>Actions</td>
        </tr>
      </thead>
      <tbody>
        {companies.map((company) => {
          return (
            <CompanyAdminComponent
              key={company.id}
              company={company}
              onChange={onChange}
            />
          );
        })}
      </tbody>
    </Table>
  );
}
