import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Loader, NavigationBar, Button } from "../../components";
import { StudentService, LoginService } from "../../services";
import NotFoundPage from "../NotFoundPage";

export default function StudentDetailsPage() {
  const studentService = new StudentService();
  const loginService = new LoginService();
  const [loading, setLoading] = useState(true);
  const [student, setStudent] = useState(null);
  const params = useParams();
  const navigate = useNavigate();

  async function getStudent() {
    await studentService.getByIdAsync(params.id).then((student) => {
      setLoading(false);
      setStudent(student);
    });
  }

  useEffect(() => {
    getStudent();
  }, []);

  async function removeStudent() {
    const userConfirmed = window.confirm("Are you sure you want to delete?");
    if (userConfirmed) {
      const result = await studentService.removeAsync(params.id);
      if (result) {
        if (student.id === loginService.getUserToken().id) {
          loginService.logOut();
          navigate("/login");
        } else {
          window.history.back();
        }
      } else {
        alert("An error occured while deleting... Please try again later!");
      }
    }
  }

  if (loading) return <Loader />;
  if (!student) return <NotFoundPage />;

  return (
    <div>
      <NavigationBar />
      <div className="container">
        <div className="text-center">
          <h1>Student Details</h1>
        </div>
        <div className="row mb-3 mt-">
          First Name: <b>{student.firstName}</b>
        </div>
        <div className="row mb-3 mt-3">
          Last Name: <b>{student.lastName}</b>
        </div>
        <div className="row mb-3 mt-">
          Email: <b>{student.email}</b>
        </div>
        <div className="row mb-3 mt-">
          Phone number: <b>{student.phoneNumber}</b>
        </div>
        <div className="row mb-3 mt-3">
          Address:{" "}
          <b>
            {student.address +
              (student.county ? ", " + student.county.name : "")}
          </b>
        </div>
        <div className="row mb-3 mt-3">
          Description: <b>{student.description}</b>
        </div>

        {student.studyArea && (
          <div className="row mb-3 mt-3">
            Study Area: <b>{student.studyArea.name}</b>
          </div>
        )}
        {(loginService.getUserToken().id === student.id ||
          sessionStorage.getItem("user_role") === "Admin") && (
          <>
            <Button
              buttonColor="secondary"
              onClick={() => {
                navigate(`/student/edit/${student.id}`);
              }}
            >
              Edit
            </Button>
            <Button buttonColor="danger" onClick={removeStudent}>
              Delete
            </Button>
          </>
        )}
      </div>
    </div>
  );
}
