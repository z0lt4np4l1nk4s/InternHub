import { Company, StudyArea } from "./index";

export class Internship {
  constructor({
    id = "",
    studyAreaId = "",
    studyArea = null,
    companyId = "",
    company = null,
    name = "",
    description = "",
    address = "",
    startDate = "",
    endDate = "",
    applicationsCount = 0,
    isApplied = false,
  }) {
    this.id = id;
    this.studyAreaId = studyAreaId;
    this.studyArea = studyArea;
    this.companyId = companyId;
    this.company = company;
    this.name = name;
    this.description = description;
    this.address = address;
    this.startDate = startDate;
    this.endDate = endDate;
    this.applicationsCount = applicationsCount;
    this.isApplied = isApplied;
  }

  static fromJson(json) {
    return new Internship({
      id: json["Id"],
      studyAreaId: json["StudyAreaId"],
      companyId: json["CompanyId"],
      name: json["Name"],
      description: json["Description"],
      address: json["Address"],
      startDate: json["StartDate"],
      endDate: json["StartDate"],
      company: json["Company"] ? Company.fromJson(json["Company"]) : null,
      studyArea: json["StudyArea"]
        ? StudyArea.fromJson(json["StudyArea"])
        : null,
      applicationsCount: json["ApplicationsCount"],
      isApplied: json["IsApplied"] ? json["IsApplied"] : false,
    });
  }
}
