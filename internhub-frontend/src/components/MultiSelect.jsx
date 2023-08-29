import React from "react";
import Select from "react-select";

export default function MultiSelect({ text, options, value, onChange }) {
  return (
    <div className="mb-3 mt-3">
      <label className="form-label">{text}</label>
      <Select options={options} isMulti value={value} onChange={onChange} />
    </div>
  );
}
