import { Student, State, Internship } from "./index";

export class InternshipApplication {
  constructor({
    id = "",
    dateCreated = null,
    dateUpdated = null,
    message = "",
    student = null,
    state = null,
    internship = null,
  }) {
    this.id = id;
    this.dateCreated = dateCreated;
    this.dateUpdated = dateUpdated;
    this.message = message;
    this.student = student;
    this.state = state;
    this.internship = internship;
  }

  static fromJson(json) {
    return new InternshipApplication({
      id: json["Id"] || "",
      dateCreated: json["DateCreated"] || "",
      dateUpdated: json["DateUpdated"] || "",
      message: json["Message"] || "",
      student: json["Student"] ? Student.fromJson(json["Student"]) : null,
      state: json["State"] ? State.fromJson(json["State"]) : null,
      internship: json["Internship"]
        ? Internship.fromJson(json["Internship"])
        : json["Internship"],
    });
  }
}
