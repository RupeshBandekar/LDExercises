import React, { Component } from 'react';

export class AccountDashboard extends Component {    
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
                            <td>{new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(this.props.account[0].overdraftLimit)}</td>
                            <td>Account status:</td>
                            <td>{this.props.account[0].accountState === 0 ? <p style={{color: 'green'}}>Active</p> : <p style={{color: 'red'}}>Blocked</p> }</td>
                        </tr>
                        <tr>
                            <td>Name:</td>
                            <td>{this.props.account[0].accountHolderName}</td>
                            <td>Daily wire transfer limit:</td>
                            <td>{new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(this.props.account[0].dailyWireTransferLimit)}</td>
                            <td>Account blocked due to:</td>
                            <td>{this.props.account[0].reasonForAccountBlock === '' ? 'NA' : this.props.account[0].reasonForAccountBlock}</td>
                        </tr>
                        <tr>
                            <td></td>
                            <td></td>
                            <td>Available balance:</td>
                            <td>
                                <p style={{color: this.props.account[0].availableFund < 0 ? 'red' : 'green'}}>
                                {new Intl.NumberFormat('en-US', {style:'currency', currency:'USD'}).format(this.props.account[0].availableFund)}
                                </p>
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}