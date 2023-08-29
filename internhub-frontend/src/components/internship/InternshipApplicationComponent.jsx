import React from "react";
import { InternshipApplicationService } from "../../services";
import Button from "../Button";

export default function InternshipApplicationComponent({
  onChange,
  internshipApplication,
}) {
  const internshipApplicationService = new InternshipApplicationService();
  const convertToShorterDate = (fullDate) => {
    const date = new Date(fullDate);
    return `${date.getFullYear()}-${(date.getMonth() + 1)
      .toString()
      .padStart(2, "0")}-${date.getDate().toString().padStart(2, "0")}`;
  };
  internshipApplication.internship.startDate = convertToShorterDate(
    internshipApplication.internship.startDate
  );
  internshipApplication.internship.endDate = convertToShorterDate(
    internshipApplication.internship.endDate
  );

  function getStudentName() {
    return (
      internshipApplication.student.firstName +
      " " +
      internshipApplication.student.lastName +
      " (" +
      internshipApplication.student.email +
      ")"
    );
  }

  async function acceptStudentAsync(isAccepted) {
    const result = await internshipApplicationService.acceptAsync(
      internshipApplication.id,
      isAccepted
    );
    if (result) {
      onChange();
    } else {
      alert(
        "An error occured while " +
          (isAccepted ? "accepting" : "declining") +
          " the student."
      );
    }
  }

  return (
    <div id="outside-container">
      <div id="internship-container">
        <div className="header-container">
          <h3>{getStudentName()}</h3>
          <div className="row text-center move-left-relative bg">
            <Button
              buttonColor={"success"}
              onClick={async () => await acceptStudentAsync(true)}
            >
              Accept
            </Button>
            <Button
              buttonColor={"danger"}
              onClick={async () => await acceptStudentAsync(false)}
            >
              Decline
            </Button>
          </div>
        </div>

        <div className="p-button-flex">
          <p id="description">{internshipApplication.message}</p>
        </div>

        <p className="duration">
          Duration: {internshipApplication.internship.startDate} -{" "}
          {internshipApplication.internship.endDate}
        </p>

        <div className="p-flex">
          <p>{internshipApplication.internship.name}</p>
          <p></p>
        </div>
      </div>
    </div>
  );
}
