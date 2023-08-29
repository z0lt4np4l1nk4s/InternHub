import React, { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import {
  InternshipApplicationService,
  InternshipService,
  LoginService,
  StudentService,
} from "../../services";
import {
  Loader,
  NavigationBar,
  RegisterPopupInternship,
  StudentsList,
} from "../../components/index";
import "../../styles/student.css";
import NotFoundPage from "../NotFoundPage";

const InternshipDetails = () => {
  const [internship, setInternship] = useState({});
  const [loading, setLoading] = useState(true);
  const { id } = useParams("");
  const [isRegisteredToInternship, setIsRegisteredToInternship] =
    useState(false);
  const [students, setStudents] = useState([]);
  const [showPopup, setShowPopup] = useState(false);
  const [isCompany, setIsCompany] = useState(false);
  const studentService = new StudentService();
  const loginService = new LoginService();
  const internshipService = new InternshipService();
  const internshipApplicationService = new InternshipApplicationService();

  const handleLogOutInternshipAsync = async () => {
    const withdraw = window.confirm(
      "Are you sure you want to withdraw your application?"
    );
    if (withdraw) {
      if (
        await internshipApplicationService.deleteByInternshipAsync(
          id,
          loginService.getUserToken().id
        )
      ) {
        alert("Application withdrawn successfully!");
        fetchIsStudentRegisteredAsync();
      } else {
        alert("Something has gone wrong, please try again!");
      }
    }
  };

  const fetchInternshipAsync = async () => {
    setInternship(await internshipService.getByIdAsync(id));
    setLoading(false);
  };

  const fetchIsStudentRegisteredAsync = async () => {
    setIsRegisteredToInternship(
      await internshipService.getIsStudentRegisteredToInternshipAsync(
        loginService.getUserToken().id,
        id
      )
    );
  };
  const handleApplySuccessAsync = async () => {
    alert("You applied successfully!");
    await fetchIsStudentRegisteredAsync();
  };

  async function checkIfCompany() {
    const role = await loginService.getUserRoleAsync();
    const isCompany =
      role.toLowerCase() === "company" || role.toLowerCase() === "admin";
    setIsCompany(isCompany);
    if (isCompany) refreshStudents();
    else fetchIsStudentRegisteredAsync();
  }

  useEffect(() => {
    checkIfCompany();
    fetchInternshipAsync();
  }, []);

  async function refreshStudents() {
    const data = await studentService.getByInternshipAsync(id);
    setStudents(data);
  }

  if (loading) return <Loader />;
  if (!internship) return <NotFoundPage />;

  return (
    <>
      <NavigationBar />
      <div className="container mt-5">
        <div className="row h-100 align-items-center justify-content-center">
          <div className="col-md-10">
            <div className="card">
              <div className="card-body">
                <h2 className="card-title text-center m-2">
                  {internship.name}
                </h2>
                <p className="card-text text-center mt-4">
                  <strong>Duration: </strong>{" "}
                  {internshipService.convertToShorterDate(internship.startDate)}{" "}
                  - {internshipService.convertToShorterDate(internship.endDate)}
                </p>
                <p className="card-text text-center">
                  {internship.description}
                </p>
                <p className="text-center">
                  We are looking for students that are in{" "}
                  {internship.studyArea ? internship.studyArea.name : ""} study
                  area.
                </p>
                <div className="row w-100">
                  <div className="col-md-12">
                    <p className="text-center mt-5">
                      <strong>Company:</strong>{" "}
                      {internship.company ? internship.company.name : ""}
                    </p>
                    <p className="text-center">
                      {" "}
                      {internship.company ? internship.company.address : ""}
                    </p>
                    <p className="text-center">
                      Check our{" "}
                      <a
                        href={
                          internship.company ? internship.company.website : ""
                        }
                        target="_blank"
                        rel="noopener noreferrer"
                      >
                        website
                      </a>{" "}
                      for more details.
                    </p>
                  </div>
                </div>
                {isRegisteredToInternship && (
                  <div className="text-center mt-5 bg-c">
                    <button
                      className="mx-auto"
                      onClick={handleLogOutInternshipAsync}
                    >
                      Withdraw your application
                    </button>
                  </div>
                )}
                {!isRegisteredToInternship && !isCompany && (
                  <div className="text-center mt-5 bg-c">
                    <button
                      className="mx-auto"
                      onClick={() => {
                        setShowPopup(true);
                      }}
                    >
                      Apply
                    </button>
                  </div>
                )}
                <RegisterPopupInternship
                  showPopup={showPopup}
                  handleApplySuccess={handleApplySuccessAsync}
                  handleClose={() => {
                    setShowPopup(false);
                  }}
                />
              </div>
            </div>
            {isCompany && (
              <>
                <div style={{ height: 40 }}></div>
                <div className="text-center">
                  <h4>Accepted students:</h4>
                </div>
                <StudentsList students={students} readonly={true} />
              </>
            )}
          </div>
        </div>
      </div>
    </>
  );
};

export default InternshipDetails;
