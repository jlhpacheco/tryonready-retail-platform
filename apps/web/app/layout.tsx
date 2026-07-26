import type { Metadata, Viewport } from "next";
import Link from "next/link";
import "./globals.css";

export const metadata: Metadata = {
  title: {
    default: "TryOnReady",
    template: "%s | TryOnReady",
  },
  description:
    "A mobile-first virtual try-on workflow foundation for independent boutiques.",
  applicationName: "TryOnReady",
  manifest: "/manifest.webmanifest",
};

export const viewport: Viewport = {
  width: "device-width",
  initialScale: 1,
  themeColor: "#402820",
  colorScheme: "light",
};

const navigation = [
  { href: "/boutique-application", label: "Boutique Application" },
  { href: "/product-readiness", label: "Product Readiness" },
  { href: "/admin-review", label: "Admin Review" },
  { href: "/consumer-try-on", label: "Consumer Try-On" },
];

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <body>
        <a className="skip-link" href="#main-content">
          Skip to main content
        </a>
        <header className="site-header">
          <div className="header-inner">
            <Link className="brand" href="/" aria-label="TryOnReady home">
              <span className="brand-mark" aria-hidden="true">
                TR
              </span>
              <span>TryOnReady</span>
            </Link>
            <nav aria-label="Primary navigation">
              <ul className="nav-list">
                {navigation.map((item) => (
                  <li key={item.href}>
                    <Link href={item.href}>{item.label}</Link>
                  </li>
                ))}
              </ul>
            </nav>
          </div>
        </header>
        <main id="main-content">{children}</main>
        <footer className="site-footer">
          <div>
            <Link className="brand footer-brand" href="/">
              TryOnReady
            </Link>
            <p>Virtual try-on for boutiques without the enterprise budget.</p>
          </div>
          <p className="footer-note">
            Private hackathon scaffold. No live YouCam requests or customer
            photographs.
          </p>
        </footer>
      </body>
    </html>
  );
}
