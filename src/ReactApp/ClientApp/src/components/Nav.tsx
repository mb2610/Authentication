import React from "react";

const Nav = () => {
  return (
    <div className="container-fluid">
    <ul className="navbar-nav me-auto mb-2 mb-lg-0">
      <li className="nav-item">
        <a className="nav-link" aria-current="page" href="/">Home</a>
      </li>
      <li className="nav-item">
        <a className="nav-link" href="/secured">Secured</a>
      </li>
    </ul>
  </div>
 );
};

export default Nav;