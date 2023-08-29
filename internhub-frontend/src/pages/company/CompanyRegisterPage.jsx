import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Button,
  Form,
  Input,
  Loader,
  NavigationBar,
  SelectDropdown,
} from "../../components";
import "../../styles/index.css";
import { CountyService } from "../../services";
import { CompanyService } from "../../services/CompanyService";
import "../../styles/nav.css";
import { Company } from "../../models";

export default function CompanyRegisterPage() {
  const [counties, setCounties] = useState([]);
  const companyService = new CompanyService();
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    fetchCounties();
  }, []);

  const fetchCounties = async () => {
    try {
      const countiesData = await new CountyService().getAsync();
      setCounties(countiesData);
    } catch (error) {
      console.error("Error fetching data", error);
    }
    setLoading(false);
  };

  if (loading) return <Loader />;

  return (
    <div>
      <NavigationBar />
      <div className="container">
        <h1 className="text-center">Register company</h1>
        <Form
          onSubmit={async (e) => {
            e.preventDefault();
            if (e.target.password.value !== e.target.confirmpassword.value) {
              alert("The two passwords don't match!");
              return;
            }
            var company = new Company({
              email: e.target.email.value,
              website: e.target.website.value,
              password: e.target.password.value,
              name: e.target.companyName.value,
              firstName: e.target.firstName.value,
              lastName: e.target.lastName.value,
              address: e.target.companyAddress.value,
              phoneNumber: e.target.phoneNumber.value,
              countyId: e.target.county.value,
              description: e.target.description.value,
            });
            console.log(company);
            const result = await companyService.postAsync(company);

            if (result) {
              navigate("/login");
            } else {
              alert(
                "An error occurred while registering your account... Please try again later!"
              );
            }
          }}
        >
          <Input name="email" text="Email:" type="email" required={true} />
          <Input
            type="password"
            name="password"
            text="Password:"
            minLength={6}
            required={true}
          />
          <Input
            type="password"
            name="confirmpassword"
            text="Confirm password:"
            minLength={6}
            required={true}
          />
          <Input name="companyName" text="Company name:" required={true} />
          <Input name="website" text="Website:" />
          <Input name="firstName" text="First name:" required={true} />
          <Input name="lastName" text="Last name:" required={true} />
          <Input name="companyAddress" text="Address:" required={true} />
          <Input name="phoneNumber" text="Phone number:" required={true} />
          <SelectDropdown
            text={"County:"}
            placeholder={"Pick county"}
            name={"county"}
            list={counties}
          />
          <Input name="description" text="Description:" required={true} />
          <br />
          <div className="text-center">
            <Button type="submit">Create</Button>
          </div>
        </Form>
      </div>
    </div>
  );
}
