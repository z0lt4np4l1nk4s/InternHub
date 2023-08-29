import { County, StudyArea } from "./index";

export class Student {
  constructor({
    id = "",
    firstName = "",
    lastName = "",
    address = "",
    email = "",
    phoneNumber = "",
    description = "",
    countyId = "",
    county = null,
    studyAreaId = "",
    studyArea = null,
    password = "",
  }) {
    this.id = id;
    this.firstName = firstName;
    this.lastName = lastName;
    this.address = address;
    this.email = email;
    this.phoneNumber = phoneNumber;
    this.description = description;
    this.countyId = countyId;
    this.county = county;
    this.studyAreaId = studyAreaId;
    this.studyArea = studyArea;
    this.password = password;
  }

  static fromJson(json) {
    return new Student({
      id: json["Id"],
      firstName: json["FirstName"],
      lastName: json["LastName"],
      address: json["Address"],
      email: json["Email"],
      phoneNumber: json["PhoneNumber"],
      description: json["Description"],
      countyId: json["County"] ? json["County"]["Id"] : null,
      county: json["County"] && County.fromJson(json["County"]),
      studyAreaId: json["StudyArea"] ? json["StudyArea"]["Id"] : null,
      studyArea: json["StudyArea"] && StudyArea.fromJson(json["StudyArea"]),
    });
  }
}
