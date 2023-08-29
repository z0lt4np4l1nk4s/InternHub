import React, { useEffect, useState } from "react";
import { InternshipApplicationFilter } from "../../models";
import { StateService } from "../../services";
import Button from "../Button";
import Form from "../Form";
import Input from "../Input";
import MultiSelect from "../MultiSelect";

export default function InternshipApplicationFilterComponent({
  onFilter,
  onClearFilter,
  filter,
}) {
  const stateService = new StateService();
  const [stateOptions, setStateOptions] = useState([]);
  const [selectedStates, setSelectedStates] = useState([]);
  const [companyName, setCompanyName] = useState(filter.companyName || "");
  const [internshipName, setInternshipName] = useState(
    filter.internshipName || ""
  );
  const [isFilterActive, setIsFilterActive] = useState(false);

  async function fetchData() {
    setIsFilterActive(
      filter.companyName ||
        filter.internshipName ||
        (filter.states && filter.states.length > 0)
    );
    const states = await stateService.getAsync();
    const selectedStates = [];
    const mappedStates = states.map((state) => {
      const data = {
        value: state.id,
        label: state.name,
      };
      if ((filter.states || []).includes(state.id)) selectedStates.push(data);
      return data;
    });
    setStateOptions(mappedStates);
    setSelectedStates(selectedStates);
  }

  useEffect(() => {
    fetchData();
  }, []);

  return (
    <div className="container row justify-content-center align-items-center">
      <Form
        onSubmit={(e) => {
          e.preventDefault();
          const filter = new InternshipApplicationFilter({
            internshipName,
            companyName,
            states: selectedStates.map((state) => state.value),
          });
          onFilter(filter);
          setIsFilterActive(true);
        }}
      >
        <div className="row">
          <div className="col-4">
            <Input
              text="Internship name:"
              name="internshipname"
              value={internshipName}
              onChange={(e) => setInternshipName(e.target.value)}
            />
          </div>
          <div className="col-4">
            <Input
              text="Company name:"
              name="companyname"
              value={companyName}
              onChange={(e) => setCompanyName(e.target.value)}
            />
          </div>
          <div className="col-4">
            <MultiSelect
              text="States:"
              options={stateOptions}
              value={selectedStates}
              onChange={(selectedStates) => setSelectedStates(selectedStates)}
            />
          </div>
        </div>
        <div className="text-center">
          <Button className="m-1" type="submit" buttonColor="primary">
            Filter
          </Button>
          {isFilterActive && (
            <Button
              className="m-1"
              buttonColor="secondary"
              onClick={() => {
                setCompanyName("");
                setInternshipName("");
                setSelectedStates([]);
                onClearFilter();
              }}
            >
              Clear
            </Button>
          )}
        </div>
      </Form>
    </div>
  );
}
