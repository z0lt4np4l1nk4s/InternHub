import React from "react";
import { Link, useNavigate } from "react-router-dom";
import Button from "../Button";
import "../../styles/nav.css";
import { LoginService } from "../../services";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import {
  faHome,
  faFileAlt,
  faGraduationCap,
  faBuilding,
  faRightFromBracket,
} from "@fortawesome/free-solid-svg-icons";
import { SmallLogo } from "../index";

export default function AdminNavigation() {
  const navigate = useNavigate();
  const handleLogout = () => {
    const loginService = new LoginService();
    if (loginService.logOut()) {
      navigate("/login");
    }
  };
  return (
    <div className="sidebar">
      <SmallLogo />
      <ul className="nav-links">
        <li className="nav-item">
          <Link className="link-style" to="/">
            <span className="icon">
              <FontAwesomeIcon className="fontawesome" icon={faHome} />
            </span>
            <span className="nav-text">Home</span>
          </Link>
        </li>
        <li className="nav-item">
          <Link className="link-style" to="/students">
            <span className="icon">
              <FontAwesomeIcon className="fontawesome" icon={faGraduationCap} />
            </span>
            <span className="nav-text">Students</span>
          </Link>
        </li>
        <li className="nav-item">
          <Link className="link-style" to="/companies">
            <span className="icon">
              <FontAwesomeIcon className="fontawesome" icon={faBuilding} />
            </span>
            <span className="nav-text">Companies</span>
          </Link>
        </li>
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
    </div>
  );
}
