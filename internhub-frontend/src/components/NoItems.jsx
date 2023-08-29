import React from "react";

export default function NoItems() {
  return (
    <div className="text-center">
      <div style={{ height: 50 }}></div>
      <img src="/images/no_data.svg" alt="No items" width={300} />
      <div style={{ height: 35 }}></div>
      <h1>No items</h1>
    </div>
  );
}
