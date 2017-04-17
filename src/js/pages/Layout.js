import React from "react";
import { Link } from "react-router";

import Footer from "../components/layout/Footer";
import NavSide from "../components/layout/NavSide";

export default class Layout extends React.Component {

  render() {
    const { location } = this.props;
    const containerStyle = {
      marginTop: "60px"
    };

    let children = null;
    if (this.props.children) {
      children = React.cloneElement(this.props.children, {
        auth: this.props.route.auth //sends auth instance to children
      })
    }

    return (
      <div>
        <div class="">
          <div class="row">
            <div class="">
              <NavSide location={location} />
            </div>
            {/*<Footer/>*/}
            <div class="main">
              {children}
            </div>
          </div>
        </div>
    </div>
    );
  }
}