import React, { FC } from "react";
import CodeBlock from "@theme/CodeBlock";
import TabItem from "@theme/TabItem";
import Tabs from "@theme/Tabs";

const cli = `dotnet add package Ubiety.Stringprep.Core`;
const packageReference = `<PackageReference Include="Ubiety.Stringprep.Core" Version="0.3.1" />`;

export const Instructions: FC = () => {
  return (
    <div style={{ padding: "10px" }}>
      <Tabs
        defaultValue="cli"
        values={[
          { label: ".NET CLI", value: "cli" },
          { label: "Package Reference", value: "reference" },
        ]}
      >
        <TabItem value="cli">
          <CodeBlock metastring="shell" className="shell">
            {cli}
          </CodeBlock>
        </TabItem>
        <TabItem value="reference">
          <CodeBlock metastring="shell" className="shell">
            {packageReference}
          </CodeBlock>
        </TabItem>
      </Tabs>
    </div>
  );
};
