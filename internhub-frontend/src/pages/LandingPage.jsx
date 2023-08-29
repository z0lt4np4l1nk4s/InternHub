import React from "react";
import "bootstrap/dist/css/bootstrap.min.css";
import "../styles/landingPage.css";
import { useNavigate } from "react-router-dom";
import { Button } from "../components";

const LandingPage = () => {
  const navigate = useNavigate();

  const handleStudentRegistry = () => {
    navigate("/student/register");
  };

  const handleCompanyRegistry = () => {
    navigate("/company/register");
  };

  const handleLogin = () => {
    navigate("/login");
  };
  return (
    <div className="d-flex flex-column align-items-center justify-content-center vh-100 vw-75">
      <div className="d-flex flex-column align-items-center">
        <img src="/logo192.png" alt="Logo" width="80" height="80" />
        <div style={{ height: 25 }}></div>
        <h1> Welcome to InternHub</h1>
      </div>
      <div className="mt-5" />
      <img src="/images/hello.svg" alt="hello" width={600} />
      <div style={{ height: 80 }}></div>
      <div className="d-flex justify-content-center w-75">
        <div>
          <Button onClick={handleStudentRegistry} width={"250px"}>
            Register as a student
          </Button>
        </div>
        <div style={{ width: 100 }}></div>
        <div>
          <Button onClick={handleCompanyRegistry} width={"250px"}>
            Register as a company
          </Button>
        </div>
      </div>
      <div style={{ height: 50 }}></div>
      <div className="row">
        <Button onClick={handleLogin} width={"250px"}>
          Login
        </Button>
      </div>
    </div>
  );
};

export default LandingPage;
