import React, { useState } from "react";
import Button from "../Button";
import Form from "../Form";
import Input from "../Input";

export default function InternshipApplicationFilter({
  onFilter,
  onClearFilter,
  filter,
}) {
  const [isFilterActive, setIsFilterActive] = useState(false);
  const [firstName, setFirstName] = useState(filter.firstName || "");
  const [lastName, setLastName] = useState(filter.lastName || "");

  return (
    <div className="container row justify-content-center align-items-center">
      <Form
        onSubmit={(e) => {
          e.preventDefault();
          const filter = {
            firstName,
            lastName,
          };
          onFilter(filter);
          setIsFilterActive(true);
        }}
      >
        <div className="row">
          <div className="col">
            <Input
              text="First name:"
              name="firstname"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
            />
          </div>
          <div className="col">
            <Input
              text="Last name:"
              name="lastname"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
            />
          </div>
        </div>
        <div className="text-center">
          <Button type="submit" buttonColor="primary">
            Filter
          </Button>
          {isFilterActive && (
            <Button
              buttonColor="secondary"
              onClick={() => {
                setFirstName("");
                setLastName("");
                onClearFilter();
              }}
            >
              Clear filter
            </Button>
          )}
        </div>
      </Form>
    </div>
  );
}
