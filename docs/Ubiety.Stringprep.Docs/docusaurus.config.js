module.exports = {
  title: "Ubiety Stringprep",
  tagline: "RFC 3454 for .NET",
  url: "https://stringprep.ubiety.dev",
  baseUrl: "/",
  onBrokenLinks: "throw",
  onBrokenMarkdownLinks: "warn",
  favicon: "img/favicon.ico",
  organizationName: "ubiety", // Usually your GitHub org/user name.
  projectName: "ubiety.stringprep.core", // Usually your repo name.
  themeConfig: {
    navbar: {
      title: "Ubiety Stringprep",
      logo: {
        alt: "Stringprep Logo",
        src: "img/rope.svg",
      },
      items: [
        {
          to: "docs/",
          activeBasePath: "docs",
          label: "Docs",
          position: "left",
        },
        { to: "blog", label: "Blog", position: "left" },
        {
          href: "https://github.com/ubiety/ubiety.stringprep.core",
          label: "GitHub",
          position: "right",
        },
      ],
    },
    footer: {
      style: "dark",
      links: [
        {
          title: "Docs",
          items: [
            {
              label: "Introduction",
              to: "docs/",
            },
          ],
        },
        {
          title: "Community",
          items: [
            {
              label: "Stack Overflow",
              href: "https://stackoverflow.com/questions/tagged/ubiety",
            },
            {
              label: "Discord",
              href: "https://discord.gg/VNtMymHdnG",
            },
          ],
        },
        {
          title: "More",
          items: [
            {
              label: "Blog",
              to: "blog",
            },
            {
              label: "GitHub",
              href: "https://github.com/ubiety/ubiety.stringprep.core",
            },
          ],
        },
      ],
      copyright: `Copyright © ${new Date().getFullYear()} Dieter (coder2000) Lunn. Built with Docusaurus.`,
    },
  },
  presets: [
    [
      "@docusaurus/preset-classic",
      {
        docs: {
          sidebarPath: require.resolve("./sidebars.js"),
          // Please change this to your repo.
          editUrl:
            "https://github.com/ubiety/Ubiety.Stringprep.Core/edit/develop/docs/Ubiety.Stringprep.Docs/",
        },
        blog: {
          showReadingTime: true,
          // Please change this to your repo.
          editUrl:
            "https://github.com/ubiety/Ubiety.Stringprep.Core/edit/develop/docs/Ubiety.Stringprep.Docs/",
        },
        theme: {
          customCss: require.resolve("./src/css/custom.css"),
        },
      },
    ],
  ],
};
