export class StudentFilter {
  constructor({ studyAreas, counties, firstName, lastName, isActive }) {
    this.studyAreas = studyAreas;
    this.counties = counties;
    this.firstName = firstName;
    this.lastName = lastName;
    this.isActive = isActive;
  }
}
