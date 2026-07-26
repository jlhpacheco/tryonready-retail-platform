import Link from "next/link";

type PlaceholderPageProps = {
  eyebrow: string;
  title: string;
  description: string;
  plannedCapabilities: readonly string[];
};

export function PlaceholderPage({
  eyebrow,
  title,
  description,
  plannedCapabilities,
}: PlaceholderPageProps) {
  return (
    <section className="placeholder-shell">
      <div className="placeholder-content">
        <div className="placeholder-copy">
          <p className="eyebrow">{eyebrow}</p>
          <h1>{title}</h1>
          <p>{description}</p>
          <p className="scaffold-note">
            This route is intentionally a scaffold placeholder. It does not
            upload photographs, submit provider requests, or publish content.
          </p>
          <Link className="back-link" href="/">
            <span aria-hidden="true">←</span>&nbsp; Back to the overview
          </Link>
        </div>
        <aside className="placeholder-panel">
          <h2>Planned for a reviewed phase</h2>
          <ul>
            {plannedCapabilities.map((capability) => (
              <li key={capability}>{capability}</li>
            ))}
          </ul>
        </aside>
      </div>
    </section>
  );
}
