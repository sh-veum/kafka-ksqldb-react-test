import { createLazyFileRoute } from "@tanstack/react-router";

export const Route = createLazyFileRoute("/about")({
  component: About,
});

function About() {
  return (
    <div className="p-2">
      <p className="text-3xl font-bold text-primary">About</p>
      <ul className="list-disc ml-4">
        <li>This is a demo application that simulates a live auction.</li>
        <li>
          Was made because I got curious with Kafka and KsqlDB and wanted to
          experiment.
        </li>
        <li>
          The frontend is built using React and Tanstack react query and router.
        </li>
        <li>
          All of the pages uses WebSockets for live updates from the related
          KsqlDB streams and tables.
        </li>
        <li>
          <strong className="text-destructive">Warning:</strong> doesn't scale
          well since KsqlDB doesn't support queries based on offsets, and the
          KsqlDB tables are unordered, meaning all auctions and chat messages
          are retrieved on pull request, no matter how many auctions there are.
        </li>
        <ul className="list-disc ml-4">
          <li>
            <i>
              A possible performance improvement could be to split the KsqlDB
              Tables and streams into multiple smaller ones exclusive to each
              auction.
            </i>
          </li>
        </ul>
      </ul>
      <p className="text-lg font-bold text-primary">Home Page</p>
      <ul className="list-disc ml-4">
        <li>Shows aa live overview over the auctions.</li>
      </ul>
      <p className="text-lg font-bold text-primary">Auction Page</p>
      <ul className="list-disc ml-4">
        <li>
          Shows a live bid feed and possibility to bid for registered users.
        </li>
        <li>Shows a live chat and possibility to chat for registered users.</li>
      </ul>
      <p className="text-lg font-bold text-primary">Admin Page</p>
      <ul className="list-disc ml-4">
        <li>Shows a graph of the bids for any individual auction</li>
        <li>Shows a feed of all bids for all auctions</li>
        <li>
          The Admin page uses{" "}
          <a
            className="text-blue-500 underline"
            href="https://recharts.org/en-US/"
            target="_blank"
          >
            recharts
          </a>{" "}
          for the graph
        </li>
      </ul>
      <div className="mt-2">
        <p className="text-base font-semibold text-primary">Admin User:</p>
        <p>Username: admin@mail.com</p>
        <p>Password: Password1!</p>
      </div>
    </div>
  );
}
