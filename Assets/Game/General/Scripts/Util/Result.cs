using System;
using static Result;

public struct UnitType { };

public abstract record ResultType();
public record Ok<T>(T Value) : ResultType;
public record Err<E>(E value) : ResultType;

public static class Result
{
    public static Result<T, E> Ok<T, E>(T value) => new(value);
    public static Result<T, E> Err<T, E>(E value) => new(value);
}

/*
        Disable non exhaustive switch warnings
*/
#pragma warning disable CS8509
#nullable enable

public class Result<T, E> 
{
    public Result(T t)
    {
        _value = new Ok<T>(t);
    }

    public Result(E e)
    {
        _value = new Err<E>(e);
    }

    ResultType _value;
  
    public bool IsOk() => this._value is Ok<T>;
    public bool IsErr() => this._value is Err<E>;
    public ResultType Case() => _value;


    public T Expect(string msg,
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)

    {
        if (_value is Err<E>)
        {
            Console.WriteLine(
                    $"Unwrap failed for Result<{typeof(T)}, {typeof(E)}>...\n" +
                    $"{msg} \n Location --> File : {sourceFilePath}, " +
                    $"function: {memberName}, line: {sourceLineNumber}\n");
            throw new BadOptionAccessException();
        }
        else if (_value is Ok<T>(var t))
            return t;

        throw new UnreachableException();
    }
    public T Unwrap(
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0)
    {

        if (_value is Err<E>(var e))
        {
            Console.WriteLine(
                    $"Error Msg: {e?.ToString() ?? "No ToString implementation for error type"}\n" +
                    $"Location --> File : {sourceFilePath}, function: {memberName}, " +
                    $"line: {sourceLineNumber}\n");
            throw new BadOptionAccessException();
        }
        else if (_value is Ok<T>(var t))
            return t;

        throw new UnreachableException();

    }
    public T UnwrapOr(T default_) => _value switch 
    {
        Err<E> => default_,
        Ok<T>(var t) => t
    };
    public T UnwrapOrElse(Func<T> f) => _value switch
    {
        Err<E> => f(),
        Ok<T>(var t) => t
    };
    public T UnwrapOrDefault()
    {
        T? default_ = default;
        return (_value, default_) switch
        {
            (_, null) => throw new TypeNotDefautableException(),
            (Err<E>, _) => default_,
            (Ok<T>(var t), _) => t,
        };

    }

    public E ExpectErr(
        string msg, 
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) 
    {
        if (_value is Ok<T>(var t))
        {
            Console.WriteLine(
                    $"{msg} \n Location --> File : {sourceFilePath}, " +
                    $"function: {memberName}, line: {sourceLineNumber}\n" +
                    $"The Ok Value: {t?.ToString() ?? "no ToString() implementation"}");
            throw new BadOptionAccessException();
        }
        else if (_value is Err<E>(var e))
            return e;

        throw new UnreachableException();
    }

    public E UnwrapErr(
        [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
        [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
        [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0) 
    {
        if (_value is Ok<T>(var t))
        {
            Console.WriteLine(
                    $"Unwrap error failed for Result<{typeof(T)}, {typeof(E)}>...\n" +
                    $"File : {sourceFilePath}, function: {memberName}, " +
                    $"line: {sourceLineNumber}\n");
            throw new BadOptionAccessException();
        }
        else if (_value is Err<E>(var e))
            return e;

        throw new UnreachableException();
    }



    public Result<U, E> Map<U>(Func<T, U> f) => _value switch
    {
        Err<E>(var e) => Err<U, E>(e),
        Ok<T>(var t) => Ok<U, E>(f(t))
    };
    public U MapOr<U>(Func<T, U> f, U default_) => _value switch
    {
        Err<E> => default_,
        Ok<T>(var t) => f(t)
    };
    public U MapOrElse<U>(Func<T, U> f, Func<E, U> fallback) => _value switch
    {
        Err<E>(var e) => fallback(e),
        Ok<T>(var t) => f(t)
    };
    public Result<T, U> MapError<U>(Func<E, U> f) => _value switch 
    {
        Err<E>(var e) => Err<T, U>(f(e)),
        Ok<T>(var t) => Ok<T, U>(t)
    };


    public Result<U, E> And<U>(Result<U, E> other) => _value switch
    {
        Err<E>(var e) => Err<U, E>(e),
        Ok<T> => other
    };
    public Result<U, E> AndThen<U>(Func<T, Result<U, E>> f) => _value switch
    {
        Err<E>(var e) => Err<U, E>(e),
        Ok<T>(var t) => f(t)
    };


    public Result<T, E> Or(Result<T, E> u) => IsOk() ? this : u;
    public Result<T, E> OrElse(Func<Result<T, E>> f) => IsOk() ? this : f();


    public bool IsOkAnd(Func<T, bool> f) => _value switch
    {
        Err<E> => false,
        Ok<T>(var t) => f(t)
    };
    public bool IsErrAnd(Func<E, bool> f) => _value switch
    {
        Err<E>(var e) => f(e),
        Ok<T> => false
    };
    

    public override int GetHashCode() => _value switch
    {
        Err<E> => 0,
        Ok<T>(var t) => t?.GetHashCode() ?? 0
    };
    public override string ToString()
    {
        if (_value is Err<E>(var e))
        {
            string msg = e?.ToString() ?? $"No ToString implementation for Err type {typeof(E)}";
            return $"Err({msg})";
        }
        else if (_value is Ok<T>(var t))
        {
            string msg = t?.ToString() ?? $"No ToString implementation for Ok type {typeof(T)}";
            return $"Ok({msg})";
        }
            
        throw new UnreachableException();
    }

    public override bool Equals(object? obj)
    {
        if (obj is Result<T, E>)
        {
            return (this, obj as Result<T, E>) switch
            {
                (Ok<T>(var t), Ok<T>(var d)) => t?.Equals(d) ?? false,
                (Err<E>, Err<E>) => true,
                _ => false
            };
        }
        return false;
    }

    public static implicit operator bool(Result<T, E> op) => op.IsOk();
    public static bool operator ==(Result<T, E> op, Result<T, E> op2) => op.Equals(op2);
    public static bool operator !=(Result<T, E> op, Result<T, E> op2) => !op.Equals(op2);

}


