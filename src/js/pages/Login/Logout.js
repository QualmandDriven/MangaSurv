import React from "react";

export default class Logout extends React.Component {
  constructor(props, context) {
    super(props, context);
    this.state = {
      profile: props.auth.getProfile(),
      auth: props.auth
    };
  }


  render() {
    const { profile, auth } = this.state;
    auth.logout();

    return (
      <div>
        <h1>Logged out!</h1>
      </div>
    );
  }
}
