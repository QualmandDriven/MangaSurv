import React from "react";
import { Link } from "react-router";

import Footer from "../components/layout/Footer";
import Nav from "../components/layout/Nav";
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
        <Nav location={location} />
        <div class="container-fluid">
          <div class="row">
            <NavSide location={location} />
            <div class="col-sm-9 col-sm-offset-3 col-md-10 col-md-offset-2 main">
              {children}
            </div>
          </div>
        </div>      
        <Footer/>
    </div>
    );
  }
}

// <div class="container" style={containerStyle}>
        //   <div class="row">
        //     <div class="col-lg-12">

        //       {this.props.children}

        //     </div>
        //   </div>