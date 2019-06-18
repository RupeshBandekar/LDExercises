import React, { Component } from 'react';

export class AccountDashboard extends Component {

    // state = {
    //     overdraftLimit: 0.00,
    // };

    // componentWillReceiveProps() {
    //     this.setState({overdraftLimit: this.props.account[0].overdraftLimit});
    // }

    render(){
        //console.log("account dashboard rendered");
        return(
            <div>
                <table className='table table-striped'>
                    <tbody>
                        <tr>
                            <td>Account Id:</td>
                            <td>{this.props.account[0].accountId}</td>
                            <td>Overdraft limit</td>
                            <td>{this.props.account[0].overdraftLimit}</td>
                            <td>Account status:</td>
                            <td>{this.props.account[0].accountState}</td>
                        </tr>
                        <tr>
                            <td>Name:</td>
                            <td>{this.props.account[0].accountHolderName}</td>
                            <td>Daily wire transfer limit:</td>
                            <td>{this.props.account[0].dailyWireTransferLimit}</td>
                            <td>Account blocked due to:</td>
                            <td>{this.props.account[0].reasonForAccountBlock}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>Available balance:</td>
                            <td>{this.props.account[0].availableFund}</td>
                            <td></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}