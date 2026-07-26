import type { MetadataRoute } from "next";

export const dynamic = "force-static";

export default function manifest(): MetadataRoute.Manifest {
  return {
    name: "TryOnReady Retail Platform",
    short_name: "TryOnReady",
    description:
      "A mobile-first virtual try-on workflow foundation for independent boutiques.",
    start_url: "/",
    display: "standalone",
    background_color: "#fffaf3",
    theme_color: "#402820",
    orientation: "portrait-primary",
  };
}
