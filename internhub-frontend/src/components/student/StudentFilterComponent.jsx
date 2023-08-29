import React, { useEffect, useState } from "react";
import { StudentFilter } from "../../models";
import { Button, CheckBox, Form, Input, Loader, MultiSelect } from "../index";
import { CountyService, StudyAreaService } from "../../services";

export default function StudentFilterComponent({
  onFilter,
  onClearFilter,
  filter,
}) {
  const studyAreaService = new StudyAreaService();
  const countyService = new CountyService();
  const [loading, setLoading] = useState(true);
  const [isActive, setIsActive] = useState(true);
  const [isFilterActive, setIsFilterActive] = useState(false);
  const [countyOptions, setCountyOptions] = useState([]);
  const [selectedCounties, setSelectedCounties] = useState([]);
  const [studyAreaOptions, setStudyAreaOptions] = useState([]);
  const [selectedStudyAreas, setSelectedStudyAreas] = useState([]);
  const [firstName, setFirstName] = useState(filter.firstName || "");
  const [lastName, setLastName] = useState(filter.lastName || "");

  useEffect(() => {
    async function fetchData() {
      setIsFilterActive(
        filter.firstName ||
          filter.lastName ||
          (filter.counties && filter.counties.length > 0) ||
          (filter.studyAreas && filter.studyAreas.length > 0)
      );
      setIsActive(filter.isActive || true);
      const counties = await countyService.getAsync();
      const studyAreas = await studyAreaService.getAsync();

      const selectedCounties = [];
      const mappedCounties = counties.map((county) => {
        const data = {
          value: county.id,
          label: county.name,
        };
        if ((filter.counties || []).includes(county.id))
          selectedCounties.push(data);
        return data;
      });
      setCountyOptions(mappedCounties);
      setSelectedCounties(selectedCounties);

      const selectedStudyAreas = [];
      const mappedStudyAreas = studyAreas.map((studyArea) => {
        const data = {
          value: studyArea.id,
          label: studyArea.name,
        };
        if ((filter.studyAreas || []).includes(studyArea.id))
          selectedStudyAreas.push(data);
        return data;
      });
      setStudyAreaOptions(mappedStudyAreas);
      setSelectedStudyAreas(selectedStudyAreas);
      setLoading(false);
    }
    fetchData();
  }, []);

  if (loading) return <Loader />;

  return (
    <div className="container row justify-content-center align-items-center">
      <Form
        onSubmit={(e) => {
          e.preventDefault();
          const filter = new StudentFilter({
            firstName: firstName,
            lastName: lastName,
            isActive: isActive,
            studyAreas: selectedStudyAreas.map((studyArea) => studyArea.value),
            counties: selectedCounties.map((county) => county.value),
          });
          onFilter(filter);
          setIsFilterActive(true);
        }}
      >
        <div className="row">
          <div className="col-4">
            {" "}
            <Input
              text="First name"
              name="firstname"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
            />
          </div>
          <div className="col-4">
            <Input
              text="Last name"
              name="lastname"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
            />
          </div>
          <div className="col-4">
            <CheckBox
              text="Active"
              name="isactive"
              checked={isActive}
              onChange={(value) => {
                setIsActive(value);
              }}
            />
          </div>
        </div>
        <div className="row">
          <div className="col-6">
            <MultiSelect
              text="Counties:"
              options={countyOptions}
              value={selectedCounties}
              onChange={(selectedCounties) =>
                setSelectedCounties(selectedCounties)
              }
            />
          </div>
          <div className="col-6">
            <MultiSelect
              text="Study areas:"
              options={studyAreaOptions}
              value={selectedStudyAreas}
              onChange={(selectedStudyAreas) =>
                setSelectedStudyAreas(selectedStudyAreas)
              }
            />
          </div>
        </div>
        <Button type="submit" buttonColor="primary">
          Filter
        </Button>
        {isFilterActive && (
          <Button
            buttonColor="secondary"
            onClick={() => {
              setIsFilterActive(false);
              setSelectedCounties([]);
              setSelectedStudyAreas([]);
              setIsActive(true);
              setFirstName("");
              setLastName("");
              onClearFilter();
            }}
          >
            Clear filter
          </Button>
        )}
      </Form>
    </div>
  );
}
