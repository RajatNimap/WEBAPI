import Navbar from "../Components/Navbar";
import Footer from "../Components/Footer";
import React from "react";
import Card from "../Components/Card";
import type { ReactNode } from "react";

interface Props {
  children: ReactNode;
}

const HomeLayout: React.FC<Props> = ({ children }) => {
  return (
    <div className="flex min-h-screen flex-col">
      {/* Sticky Navbar */}
      <header className="sticky top-0 z-50">
        <Navbar />
      </header>

      {/* Page content grows in middle */}
      <main className="flex-1">
        
        <Card />
        {children}</main>

      {/* Footer sticks to bottom */}
      <footer className="mt-auto">
        <Footer />
      </footer>
    </div>
  );
};

export default HomeLayout;
