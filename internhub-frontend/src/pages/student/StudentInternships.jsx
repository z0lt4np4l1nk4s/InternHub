import React from "react";
import { useState } from "react";
import { useEffect } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import {
  InternshipApplicationFilterComponent,
  InternshipComponent,
  NavigationBar,
  NoItems,
  Paging,
} from "../../components";
import { PagedList } from "../../models";
import { InternshipApplicationService, LoginService } from "../../services";

export default function StudentInternships() {
  const internshipApplicationService = new InternshipApplicationService();
  const loginService = new LoginService();
  const navigate = useNavigate();
  const [searchParams, setSearchParams] = useSearchParams();
  const [currentFilter, setCurrentFilter] = useState({
    pageNumber: +(searchParams.get("pageNumber") ?? 1),
    companyName: searchParams.get("companyName") || "",
    internshipName: searchParams.get("internshipName") || "",
    states: searchParams.getAll("states"),
  });
  const [pagedInternshipApplications, setPagedInternshipApplication] = useState(
    new PagedList({})
  );

  async function getInternshipApplications() {
    const data = await internshipApplicationService.getAsync({
      ...currentFilter,
      sortBy: "Id",
      sortOrder: "ASC",
      pageSize: 3,
      studentId: loginService.getUserToken().id,
    });
    setPagedInternshipApplication(data);
  }

  useEffect(() => {
    getInternshipApplications();
  }, [searchParams, currentFilter]);

  return (
    <div>
      <NavigationBar />
      <div className="container">
        <div className="text-center">
          <h1>My applied internships</h1>
        </div>
        <InternshipApplicationFilterComponent
          filter={currentFilter}
          onFilter={(filter) => {
            setSearchParams({ ...filter, pageNumber: 1 });
            setCurrentFilter({ ...filter, pageNumber: 1 });
          }}
          onClearFilter={() => {
            setSearchParams({ pageNumber: 1 });
            setCurrentFilter({ pageNumber: 1 });
          }}
        />
        <div style={{ height: 30 }}></div>
        {pagedInternshipApplications.listSize === 0 && <NoItems />}
        {pagedInternshipApplications.data.map((internshipApplication) => (
          <InternshipComponent
            key={internshipApplication.id}
            internship={internshipApplication.internship}
            buttonText={"Details"}
            hasApplicationsCount={false}
            showState={true}
            state={internshipApplication.state.name}
            redirectTo={() => {
              navigate(
                `/internship/details/${internshipApplication.internship.id}`
              );
            }}
          />
        ))}
        <Paging
          currentPage={pagedInternshipApplications.currentPage}
          lastPage={pagedInternshipApplications.lastPage}
          onPageChanged={(page) => {
            setSearchParams({ ...currentFilter, pageNumber: page });
            setCurrentFilter({ ...currentFilter, pageNumber: page });
          }}
        />
      </div>
    </div>
  );
}
