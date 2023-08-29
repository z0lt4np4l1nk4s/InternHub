import React, { useState } from "react";
import Button from "../Button";
import Form from "../Form";
import Input from "../Input";

export default function CompanyFilterComponent({
  onFilter,
  onClearFilter,
  filter,
}) {
  const [isFilterActive, setIsFilterActive] = useState(false);
  const [name, setName] = useState(filter.name || "");
  return (
    <div className="container row justify-content-center align-items-center">
      <Form
        onSubmit={(e) => {
          e.preventDefault();
          const filter = {
            name: name,
          };
          onFilter(filter);
          setIsFilterActive(true);
        }}
      >
        <div className="col">
          <Input
            text="Company Name"
            name="name"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>
        <Button type="submit" buttonColor="primary">
          Filter
        </Button>
        {isFilterActive && (
          <Button
            buttonColor="secondary"
            onClick={() => {
              setName("");
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
