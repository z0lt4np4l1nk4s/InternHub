import React from "react";
import "../../styles/Company.css";
import { useState, useEffect } from "react";
import { Button, Input, NavigationBar, SelectDropdown } from "../../components";
import { Form } from "../../components";
import { InternshipService, StudyAreaService } from "../../services";
import { useNavigate } from "react-router";
import { Internship } from "../../models";

export default function InternshipCreate() {
  const internshipService = new InternshipService();
  const [studyAreas, setStudyAreas] = useState([]);
  const [studyAreaId, setStudyAreaId] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    fetchStudyAreas();
  }, []);

  const fetchStudyAreas = async () => {
    try {
      const studyAreasData = await new StudyAreaService().getAsync();
      setStudyAreas(studyAreasData);
    } catch (error) {
      console.log("Unable to fetch study areas.", error);
    }
  };
  const handleClick = () => {
    navigate("/");
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    const internship = new Internship({
      name: e.target.name.value,
      description: e.target.description.value,
      address: e.target.address.value,
      startDate: e.target.startdate.value,
      endDate: e.target.enddate.value,
      studyAreaId: studyAreaId,
    });
    const result = await internshipService.postAsync(internship);
    if (result) {
      navigate("/");
    } else {
      alert(
        "An error occured while creating the internship... Please try again later!"
      );
    }
  };

  return (
    <div>
      <NavigationBar />
      <div className="container">
        <h1 className="text-center">New internship</h1>
        <Form onSubmit={handleSubmit}>
          <div className="row justify-content-center mt-3">
            <div className="col-md-6">
              <Input text={"Name:"} name={"name"} type={"text"} />
            </div>
          </div>

          <div className="row justify-content-center mt-3">
            <div className="col-md-6">
              <Input text={"Description:"} name={"description"} type={"text"} />
            </div>
          </div>

          <div className="row justify-content-center mt-3">
            <div className="col-md-6">
              <Input text={"Address:"} name={"address"} type={"text"} />
            </div>
          </div>

          <div className="row justify-content-center mt-3">
            <div className="col-md-3">
              <Input text={"Start Date:"} name={"startdate"} type={"date"} />
            </div>

            <div className="col-md-3">
              <Input text={"End Date:"} name={"enddate"} type={"date"} />
            </div>
          </div>

          <div className="row justify-content-center mt-3">
            <div className="col-md-6">
              <SelectDropdown
                placeholder={"Select study area"}
                name={"studyArea"}
                list={studyAreas}
                text="Study Area:"
                selectedId={studyAreaId}
                onChange={(e) => setStudyAreaId(e.target.value)}
              />
            </div>
          </div>

          <div className="mt-4">
            <div className="d-flex justify-content-end w-50">
              <Button type="submit" buttonColor="primary">
                Create
              </Button>
              {/* <button
            type="submit"
            className="btn btn-primary custom-button"
            onClick={handleClick}
          >
            Submit
          </button> */}
            </div>
          </div>
        </Form>
      </div>
    </div>
  );
}
