export class State {
  constructor({ id = "", name = "" }) {
    this.id = id;
    this.name = name;
  }

  static fromJson(json) {
    return new State({ id: json["Id"], name: json["Name"] });
  }
}
