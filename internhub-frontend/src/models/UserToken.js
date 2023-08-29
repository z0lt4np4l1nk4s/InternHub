export class UserToken {
  constructor(data) {
    this.id = data.id;
    this.token = data.token;
    this.fullName = data.fullName;
    this.email = data.email;
    this.expires = new Date(data.expires);
    this.role = data.role;
  }

  static fromResponse(json) {
    return new UserToken({
      id: json.userId,
      token: json.access_token,
      fullName: json.fullName,
      email: json.email,
      expires: json[".expires"],
      role: json.role,
    });
  }
}
