import React from "react";
import { Link, useNavigate } from "react-router-dom";
import Button from "../Button";
import "../../styles/nav.css";
import { LoginService } from "../../services";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faRightFromBracket } from "@fortawesome/free-solid-svg-icons";
import { SmallLogo } from "../index";

export default function UserNavigation() {
  const loginService = new LoginService();
  const navigate = useNavigate();
  const handleLogout = () => {
    if (loginService.logOut()) {
      navigate("/login");
    }
  };

  return (
    <div className="sidebar">
      <SmallLogo />
      {loginService.isUserTokenValid() && (
        <ul className="nav-links">
          <li className="nav-item">
            <Link className="link-style" to="/login" onClick={handleLogout}>
              <span className="icon">
                <FontAwesomeIcon
                  className="fontawesome"
                  icon={faRightFromBracket}
                />
              </span>
              <span className="nav-text">Logout</span>
            </Link>
          </li>
        </ul>
      )}
    </div>
  );
}
