import React, { useEffect, useState } from "react";
import {
  Paging,
  InternshipFilter,
  NavigationBar,
  NoItems,
  InternshipComponent,
} from "../../components";
import "../../styles/student.css";
import { PagedList } from "../../models";
import { InternshipService, LoginService } from "../../services";
import { useNavigate } from "react-router-dom";
import { useSearchParams } from "react-router-dom/dist";

export default function StudentHomePage() {
  const [pagedInternships, setPagedInternships] = useState(new PagedList({}));
  const [searchParams, setSearchParams] = useSearchParams();
  const [filterData, setFilterData] = useState({
    pageNumber: +(searchParams.get("pageNumber") ?? 1),
    name: searchParams.get("name") || "",
    endDate: searchParams.get("endDate") || "",
    startDate: searchParams.get("startDate") || "",
    counties: searchParams.getAll("counties"),
  });
  const internshipService = new InternshipService();
  const loginService = new LoginService();

  const navigate = useNavigate();

  const refreshInternships = async () => {
    const data = await internshipService.getAsync({
      ...filterData,
      sortBy: "Id",
      sortOrder: "ASC",
      pageSize: 3,
    });
    console.log(data);
    setPagedInternships(data);
  };

  useEffect(() => {
    refreshInternships();
  }, [searchParams, filterData]);

  return (
    <div className="container">
      <NavigationBar />
      <InternshipFilter
        filter={filterData}
        onFilter={(filter) => {
          setSearchParams({ ...filter, pageNumber: 1 });
          setFilterData({ ...filter, pageNumber: 1 });
        }}
        onClearFilter={() => {
          setSearchParams({ pageNumber: 1 });
          setFilterData({ pageNumber: 1 });
        }}
      />
      {pagedInternships.listSize === 0 && <NoItems />}
      {pagedInternships.data.map((internship) => (
        <InternshipComponent
          key={internship.id}
          internship={internship}
          buttonText={"Details"}
          hasApplicationsCount={false}
          isApplied={internship.isApplied}
          redirectTo={() => {
            navigate(`/internship/details/${internship.id}`);
          }}
        />
      ))}
      <Paging
        currentPage={pagedInternships.currentPage}
        lastPage={pagedInternships.lastPage}
        onPageChanged={(page) => {
          setSearchParams({ ...filterData, pageNumber: page });
          setFilterData({ ...filterData, pageNumber: page });
        }}
      />
    </div>
  );
}
