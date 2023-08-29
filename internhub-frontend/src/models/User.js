export class User {
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
    this.password = password;
  }
}
