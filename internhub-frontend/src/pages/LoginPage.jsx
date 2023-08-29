import React from "react";
import { useNavigate } from "react-router-dom";
import { Button, Input, NavigationBar } from "../components";
import { LoginService } from "../services";

export default function LoginPage() {
  const loginService = new LoginService();
  const navigate = useNavigate();

  async function onSubmit(e) {
    e.preventDefault();
    const result = await loginService.loginAsync({
      username: e.target.email.value,
      password: e.target.password.value,
    });
    if (result) navigate("/");
    else alert("Invalid email or password!");
  }

  return (
    <div>
      <div className="vh-100 d-flex justify-content-center align-items-center">
        <NavigationBar />
        <div className="container">
          <div className="row justify-content-center">
            <div className="col">
              <div className="col d-flex justify-content-center">
                <img id="loginimg" src="/images/login.svg" alt="login_image" />
              </div>
              <div className="text-center mb-3">
                <h1>Login</h1>
              </div>
              <form className="form" onSubmit={onSubmit}>
                <div className="mb-3">
                  <Input
                    type="email"
                    required={true}
                    name="email"
                    text="Email:"
                    pattern={"[a-zA-Z0-9.-_]+@[a-zA-Z.-]{2,}.[a-zA-Z]{2,}"}
                  />
                </div>
                <div className="mb-3">
                  <Input
                    type="password"
                    required={true}
                    name="password"
                    text="Password:"
                  />
                </div>
                <div className="text-center">
                  <Button type={"submit"} buttonColor={"primary"}>
                    Login
                  </Button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
