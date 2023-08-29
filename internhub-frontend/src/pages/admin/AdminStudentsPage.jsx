import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  NavigationBar,
  NoItems,
  Paging,
  StudentFilterComponent,
  StudentsList,
} from "../../components";
import { useSearchParams } from "react-router-dom/dist";
import { PagedList } from "../../models";
import { StudentService } from "../../services";

export default function AdminStudentsPage() {
  const studentService = new StudentService();
  const [searchParams, setSearchParams] = useSearchParams();
  const [pagedStudents, setPagedStudents] = useState(new PagedList({}));
  const [currentFilter, setCurrentFilter] = useState({
    pageNumber: +(searchParams.get("pageNumber") ?? 1),
    firstName: searchParams.get("firstName"),
    lastName: searchParams.get("lastName"),
    isActive:
      searchParams.get("isActive") === null
        ? true
        : searchParams.get("isActive").toLowerCase() === "true",
    studyAreas: searchParams.getAll("studyAreas"),
    counties: searchParams.getAll("counties"),
  });
  const navigate = useNavigate();

  async function refreshStudents() {
    const data = await studentService.getAdminAsync({
      ...currentFilter,
      sortBy: "Id",
      sortOrder: "ASC",
      pageSize: 10,
    });
    setPagedStudents(data);
  }

  useEffect(() => {
    refreshStudents();
  }, [currentFilter, searchParams]);

  return (
    <div>
      <NavigationBar />
      <div className="container">
        <div className="text-center">
          <h1>Students</h1>
        </div>
        <StudentFilterComponent
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
        <StudentsList
          students={pagedStudents.data}
          onEdit={(id) => {
            navigate(`/student/edit/${id}`);
          }}
          onRemove={() => {
            setSearchParams({
              ...currentFilter,
              pageNumber: pagedStudents.currentPage,
            });
          }}
          readonly={false}
          isActive={currentFilter.isActive}
        />
        {pagedStudents.listSize === 0 && <NoItems />}
        <Paging
          currentPage={pagedStudents.currentPage}
          lastPage={pagedStudents.lastPage}
          onPageChanged={(page) => {
            setSearchParams({ ...currentFilter, pageNumber: page });
            setCurrentFilter({ ...currentFilter, pageNumber: page });
          }}
        />
      </div>
    </div>
  );
}
