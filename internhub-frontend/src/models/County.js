export class County {
  constructor({ id = "", name = "" }) {
    this.id = id;
    this.name = name;
  }

  static fromJson(json) {
    return new County({ id: json["Id"], name: json["Name"] });
  }
}
