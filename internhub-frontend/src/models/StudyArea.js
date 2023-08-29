export class StudyArea {
  constructor({ id = "", name = "" }) {
    this.id = id;
    this.name = name;
  }

  static fromJson(json) {
    return new StudyArea({ id: json["Id"], name: json["Name"] });
  }
}
