import React from "react";
import { useNavigate } from "react-router-dom";
import "../../styles/student.css";
import { StateComponent, AppliedComponent, Button } from "../index";

export default function InternshipComponent({
  buttonText,
  internship,
  hasApplicationsCount,
  redirectTo,
  showState,
  state,
  isApplied,
  isOwner,
}) {
  const navigate = useNavigate();
  const convertToShorterDate = (fullDate) => {
    const date = new Date(fullDate);
    return `${date.getFullYear()}-${(date.getMonth() + 1)
      .toString()
      .padStart(2, "0")}-${date.getDate().toString().padStart(2, "0")}`;
  };
  internship.startDate = convertToShorterDate(internship.startDate);
  internship.endDate = convertToShorterDate(internship.endDate);

  return (
    <div id="outside-container">
      <div id="internship-container">
        <div className="header-container">
          <h3>{internship.name}</h3>
          {showState && <StateComponent state={state} />}
          {isApplied && <AppliedComponent />}
        </div>

        <div className="p-button-flex">
          <p id="description">{internship.description}</p>
          <div className="bg text-center">
            <Button onClick={() => redirectTo()}>{buttonText}</Button>
            {isOwner && (
              <Button
                buttonColor={"secondary"}
                onClick={() => {
                  navigate("/internship/edit/" + internship.id);
                }}
              >
                Edit
              </Button>
            )}
          </div>
        </div>

        <p className="duration">
          Duration: {internship.startDate} - {internship.endDate}
        </p>

        <div className="p-flex">
          <p>{internship.company.name}</p>
          <p>{internship.company.address}</p>
          {hasApplicationsCount && (
            <p>Number of applications: {internship.applicationsCount}</p>
          )}
        </div>
      </div>
    </div>
  );
}
