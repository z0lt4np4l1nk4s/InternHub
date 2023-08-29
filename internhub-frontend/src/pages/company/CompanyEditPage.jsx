import React, { useEffect, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import {
  Form,
  Input,
  Loader,
  SelectDropdown,
  CompanyNavigation,
  Button,
} from "../../components";
import { CountyService, CompanyService, LoginService } from "../../services";
import NotFoundPage from "../NotFoundPage";

export default function CompanyEditPage() {
  const [counties, setCounties] = useState([]);
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const companyService = new CompanyService();
  const countyService = new CountyService();
  const loginService = new LoginService();
  const params = useParams();

  const [company, setCompany] = useState({
    email: "",
    companyName: "",
    website: "",
    firstName: "",
    lastName: "",
    companyAddress: "",
    phoneNumber: "",
    countyId: "",
    description: "",
  });

  useEffect(() => {
    getCompany();
  }, []);

  async function getCompany() {
    try {
      const companyData = await companyService.getByIdAsync(params.id);
      console.log(companyData);
      const countiesData = await countyService.getAsync();
      setCounties(countiesData);
      setCompany(companyData);
    } catch (error) {
      console.error("Error fetching company data", error);
    }
    setLoading(false);
  }

  async function handleFormSubmit(event) {
    event.preventDefault();
    try {
      const success = await companyService.updateAsync(params.id, company);
      if (success) {
        navigate(`/company/details/${params.id}`);
      } else {
        alert("An error occured while updating... Please try again later!");
      }
    } catch (error) {
      console.error("Error updating company data", error);
    }
  }
  if (loading) return <Loader />;
  if (!company) return <NotFoundPage />;

  return (
    <div>
      <CompanyNavigation />
      <div className="container row">
        <h1 className="text-center">Edit Company</h1>

        <Form onSubmit={handleFormSubmit}>
          <Input
            name="companyName"
            text="Company name:"
            value={company.name}
            onChange={(e) => setCompany({ ...company, name: e.target.value })}
          />
          <Input
            name="website"
            text="Website:"
            value={company.website}
            onChange={(e) =>
              setCompany({ ...company, website: e.target.value })
            }
          />
          <Input
            name="firstName"
            text="First name:"
            value={company.firstName}
            onChange={(e) =>
              setCompany({ ...company, firstName: e.target.value })
            }
          />
          <Input
            name="lastName"
            text="Last name:"
            value={company.lastName}
            onChange={(e) =>
              setCompany({ ...company, lastName: e.target.value })
            }
          />
          <Input
            name="companyAddress"
            text="Address:"
            value={company.address}
            onChange={(e) =>
              setCompany({ ...company, address: e.target.value })
            }
          />
          <Input
            name="phoneNumber"
            text="Phone number:"
            value={company.phoneNumber}
            onChange={(e) =>
              setCompany({ ...company, phoneNumber: e.target.value })
            }
          />
          <SelectDropdown
            text={"County:"}
            placeholder={"Pick county"}
            name={"county"}
            list={counties}
            selectedId={company.countyId}
            onChange={(e) =>
              setCompany({ ...company, countyId: e.target.value })
            }
          />
          <Input
            name="description"
            text="Description:"
            value={company.description}
            onChange={(e) =>
              setCompany({ ...company, description: e.target.value })
            }
          />
          <br />
          {(params.id === loginService.getUserToken().id ||
            sessionStorage.getItem("user_role") === "Admin") && (
            <Button type="submit" buttonColor="primary">
              Save
            </Button>
          )}
        </Form>
      </div>
    </div>
  );
}
