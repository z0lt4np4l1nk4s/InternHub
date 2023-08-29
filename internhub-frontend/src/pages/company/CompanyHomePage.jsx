import React, { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import {
  NavigationBar,
  InternshipFilter,
  NoItems,
  Paging,
  Button,
  InternshipComponent,
} from "../../components";
import { PagedList } from "../../models";
import { InternshipService, LoginService } from "../../services";

export default function CompanyHomePage() {
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
      companyId: loginService.getUserToken().id,
      pageSize: 3,
    });
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
      <div className="text-center">
        <Button
          onClick={() => {
            navigate("/internship/create");
          }}
        >
          Create new internship
        </Button>
      </div>
      {pagedInternships.listSize === 0 && <NoItems />}
      {pagedInternships.data.map((internship) => (
        <InternshipComponent
          key={internship.id}
          internship={internship}
          buttonText={"Details"}
          hasApplicationsCount={true}
          isOwner={true}
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
