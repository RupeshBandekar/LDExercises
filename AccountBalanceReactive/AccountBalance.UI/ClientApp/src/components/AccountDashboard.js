import React, { Component } from 'react';

export class AccountDashboard extends Component {

    // state = {
    //     overdraftLimit: 0.00,
    // };

    // componentWillReceiveProps() {
    //     this.setState({overdraftLimit: this.props.account[0].overdraftLimit});
    // }

    render(){
        //console.log("account dashboard");
        return(
            <div>
                <table className='table table-striped'>
                    <tbody>
                        <tr>
                            <td>Account Id:</td>
                            <td>{this.props.account[0].accountId}</td>
                            <td>Available balance:</td>
                            <td>{this.props.account[0].availableFund}</td>
                            <td>{this.props.account[0].overdraftLimit}</td>
                        </tr>
                        <tr>
                            <td>Name:</td>
                            <td>{this.props.account[0].accountHolderName}</td>
                            <td>Account status:</td>
                            <td>{this.props.account[0].accountState}</td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}